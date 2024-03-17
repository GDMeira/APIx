import { faker } from '@faker-js/faker';
import fs from 'fs';
import dotenv from 'dotenv';
import knex from 'knex';
import { v4 as uuidv4 } from "uuid";

dotenv.config();

const knexx = new knex({
    client: "pg",
    connection: process.env.DATABASE_URL,
});

const PIXKEYS = 1_000_000;
const ERASE_DATA = false;

async function run() {
    if (ERASE_DATA) {
        await knexx("PixKey").del();
        await knexx("PaymentProviderAccount").del();
        await knexx("Payment").del();
        await knexx("User").del();
        await knexx("PaymentProvider").del();
    }

    const start = new Date();

    // PaymentProviderAccount
    const account = await generateAccount();
    await populateAccount(account);
    generateJson("./seed/existing_payment_provider_accounts.json", account);

    // pixKey
    const pixKey = await generatePixKey(account);
    await populatePixKey(pixKey);
    generateJson("./seed/existing_pix_keys.json", pixKey);

    console.log("Closing DB connection...");
    await knexx.destroy();

    const end = new Date();

    console.log("Done!");
    console.log(`Finished in ${(end - start) / 1000} seconds`);
}

run();

async function generateAccount() {
    console.log(`Generating ${PIXKEYS} accounts...`);
    const accounts = [];

    const users = await knexx.select('Id').table('User');
    const psps = await knexx.select('Id').table('PaymentProvider');

    for (let i = 0; i < PIXKEYS; i++) {
        const randomUserId = users[Math.floor(Math.random() * users.length)];
        const randomPspId = psps[Math.floor(Math.random() * psps.length)];

        accounts.push({
            Number: `${faker.finance.accountNumber()}`,
            Agency: `${faker.finance.accountNumber()}`,
            UserId: randomUserId.Id,
            PaymentProviderId: randomPspId.Id
        });
    }

    return accounts;
}

async function generatePixKey() {
    console.log(`Generating ${PIXKEYS} pix keys...`);
    const pixKey = [];
    const accounts = await knexx.select('Id').table('PaymentProviderAccount');

    for (let i = 0; i < PIXKEYS; i++) {
        const randomAccountId = accounts[Math.floor(Math.random() * accounts.length)];
        pixKey.push(generateRandomKey(randomAccountId.Id));
    }

    return pixKey;
}

async function populatePixKey(pixKeys) {
    console.log("Storing on DB...");

    const tableName = "PixKey";
    await knexx.batchInsert(tableName, pixKeys);
}

async function populateAccount(accounts) {
    console.log("Storing on DB...");

    const tableName = "PaymentProviderAccount";
    await knexx.batchInsert(tableName, accounts);
}

function generateJson(filepath, data) {
    if (fs.existsSync(filepath)) {
        fs.unlinkSync(filepath);
    }
    fs.writeFileSync(filepath, JSON.stringify(data));
}

function generateRandomKey(accountId) {
    const types = ['Email', 'Phone', 'Random'];
    const type = types[Math.floor(Math.random() * types.length)];
    let value = '';

    if (type === 'Email') {
        value = `${faker.lorem.word() + faker.number.int({min: 1, max: 9999999}) + 
            faker.internet.email()}`;
    } else if (type === 'Phone') {
        value = generatePhone();
    } else {
        value = uuidv4();
    }

    function generatePhone() {
        let phone = '+';
        for (let i = 0; i < 13; i++) {
            phone += Math.floor(Math.random() * 9) + 1;
        }

        return phone;
    }

    return { Type: type, Value: value, PaymentProviderAccountId: accountId }
}