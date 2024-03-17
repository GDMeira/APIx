import fs from "fs";
import dotenv from "dotenv";
import knex from "knex";

dotenv.config();

const knexx = new knex({
  client: "pg",
  connection: process.env.DATABASE_URL,
});

const USERS = 1_000_000;
const ERASE_DATA = true;

async function run() {
  if (ERASE_DATA) {
    await knexx("PixKey").del();
    await knexx("PaymentProviderAccount").del();
    await knexx("Payment").del();
    await knexx("User").del();
    await knexx("PaymentProvider").del();
  }

  const start = new Date();

  // users
  const users = generateUsers();
  await populateUsers(users);
  generateJson("./seed/existing_users.json", users);

  console.log("Closing DB connection...");
  await knexx.destroy();

  const end = new Date();

  console.log("Done!");
  console.log(`Finished in ${(end - start) / 1000} seconds`);
}

run();

function generateUsers() {
  console.log(`Generating ${USERS} users...`);
  const users = [];
  for (let i = 0; i < USERS; i++) {
    users.push({
      Name: `${Date.now() + i}`,
      Cpf: (1 * 10**10 + i).toString(),
    });
  }

  return users;
}

async function populateUsers(users) {
  console.log("Storing on DB...");

  const tableName = "User";
  await knexx.batchInsert(tableName, users);
}

function generateJson(filepath, data) {
  if (fs.existsSync(filepath)) {
    fs.unlinkSync(filepath);
  }
  fs.writeFileSync(filepath, JSON.stringify(data));
}