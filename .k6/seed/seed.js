const dotenv = require("dotenv");
const fs = require("fs");
// const { faker } = require("@faker-js/faker");

dotenv.config();

const knex = require("knex")({
  client: "pg",
  connection: process.env.DATABASE_URL,
});

const USERS = 1_000_000;
const ERASE_DATA = false;

async function run() {
  if (ERASE_DATA) {
    await knex("User").del();
  }

  const start = new Date();

  // users
  const users = generateUsers();
  await populateUsers(users);
  generateJson("./seed/existing_users.json", users);

  console.log("Closing DB connection...");
  await knex.destroy();

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
      Cpf: (1 * 10^11 + i).toString(),
    });
  }

  return users;
}

async function populateUsers(users) {
  console.log("Storing on DB...");

  const tableName = "User";
  await knex.batchInsert(tableName, users);
}

function generateJson(filepath, data) {
  if (fs.existsSync(filepath)) {
    fs.unlinkSync(filepath);
  }
  fs.writeFileSync(filepath, JSON.stringify(data));
}