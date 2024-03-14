const dotenv = require("dotenv");
const fs = require("fs");
const { faker } = require("@faker-js/faker");
const { v4: uuidv4 } = require('uuid');

dotenv.config();

const knex = require("knex")({
  client: "pg",
  connection: process.env.DATABASE_URL,
});

const PAYMENTPROVIDERS = 1_000_000;
const ERASE_DATA = true;

async function run() {
  if (ERASE_DATA) {
    await knex("PaymentProvider").del();
  }

  const start = new Date();

  // users
  const paymentProviders = generatePaymentProviders();
  await populatePaymentProviders(paymentProviders);
  generateJson("./seed/existing_payment_providers.json", paymentProviders);

  console.log("Closing DB connection...");
  await knex.destroy();

  const end = new Date();

  console.log("Done!");
  console.log(`Finished in ${(end - start) / 1000} seconds`);
}

run();

function generatePaymentProviders() {
  console.log(`Generating ${PAYMENTPROVIDERS} providers...`);
  const providers = [];
  for (let i = 0; i < PAYMENTPROVIDERS; i++) {
    providers.push({
      Name: faker.lorem.word(),
      Token: uuidv4(),
      PatchPaymentUrl: "/payments/pix",
      PostPaymentUrl: "/payments/pix"
    });
  }

  return providers;
}

async function populatePaymentProviders(providers) {
  console.log("Storing on DB...");

  const tableName = "PaymentProvider";
  await knex.batchInsert(tableName, providers);
}

function generateJson(filepath, data) {
  if (fs.existsSync(filepath)) {
    fs.unlinkSync(filepath);
  }
  fs.writeFileSync(filepath, JSON.stringify(data));
}