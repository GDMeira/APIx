import { faker } from '@faker-js/faker';
import fs from 'fs';
import dotenv from 'dotenv';
import knex from 'knex';

dotenv.config();

const knexx = new knex({
    client: "pg",
    connection: process.env.DATABASE_URL,
});

const PAYMENTS = 1_000_000;
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

    // Payments
    const payments = await generatePayment();
    await populatePayment(payments);
    generateJson("./seed/existing_payments.json", payments);
    generateNDJSON("./seed/existing_payments.ndjson", payments);

    console.log("Closing DB connection...");
    await knexx.destroy();

    const end = new Date();

    console.log("Done!");
    console.log(`Finished in ${(end - start) / 1000} seconds`);
}

run();

async function generatePayment() {
    console.log(`Generating ${PAYMENTS} payments...`);
    const payments = [];

    const pixKeyIds = await knexx.select('Id').table('PixKey');
    const PaymentProviderAccountIds = await knexx.select('Id').table('PaymentProviderAccount');
    const randomKeyId = pixKeyIds[Math.floor(Math.random() * pixKeyIds.length)];
    const randomAccountId = PaymentProviderAccountIds[Math.floor(Math.random() * PaymentProviderAccountIds.length)];
    const status = ['FAILED', 'SUCCESS'];

    for (let i = 0; i < PAYMENTS; i++) {
        const randomStatus = status[Math.floor(Math.random() * status.length)];

        payments.push({
            Amount: `${faker.number.int({min: 1, max: 9999999})}`,
            Description: `${faker.lorem.sentence(3)}`,
            PixKeyId: randomKeyId.Id,
            PaymentProviderAccountId: randomAccountId.Id,
            Status: randomStatus
        });
    }

    return payments;
}

async function populatePayment(payments) {
    console.log("Storing on DB...");

    const tableName = "Payment";
    await knexx.batchInsert(tableName, payments);
}

function generateJson(filepath, data) {
    if (fs.existsSync(filepath)) {
        fs.unlinkSync(filepath);
    }
    fs.writeFileSync(filepath, JSON.stringify(data));
}

function generateNDJSON(filepath, data) {
    if (fs.existsSync(filepath)) {
        fs.unlinkSync(filepath);
    }

    const stream = fs.createWriteStream(filepath, { flags: 'a' });

    data.forEach(obj => {
        const newObj = {id: obj.Id, status: obj.Status}
        stream.write(JSON.stringify(newObj) + '\n');
    });

    stream.end();
}