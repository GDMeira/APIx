import http from 'k6/http';
import { SharedArray } from 'k6/data';

export const options = {
    vus: 100, //virtual users
    duration: '20s'
}

const data = new SharedArray('users', () => JSON.parse(open("../seed/existing_users.json")));
const pspData = new SharedArray('PSPs', () => JSON.parse(open("../seed/existing_payment_providers.json")));
const pixKeys = new SharedArray('pixKeys', () => JSON.parse(open("../seed/existing_pix_keys.json")));

export default function () {
    const randomUserOrigin = data[Math.floor(Math.random() * data.length)];
    const randomUserDestiny = data[Math.floor(Math.random() * data.length)];
    const randomPsp = pspData[Math.floor(Math.random() * pspData.length)];
    const randomPixKey = pixKeys[Math.floor(Math.random() * pixKeys.length)];

    const body = JSON.stringify({
        origin: {
            user: {
                cpf: randomUserOrigin.Cpf,
            },
            account: {
                number: `${Date.now() + randomUserOrigin.Cpf}`,
                agency: `${Date.now() + randomUserOrigin.Name}`,
            }
        },
        destiny: {
            key: {
                type: randomPixKey.Type,
                value: randomPixKey.Value
            }
        },
        amount: Math.floor(Math.random() * 10000) + 1,
        description: `Payment from ${randomUserOrigin.Name} to ${randomUserDestiny.Name}`
    })

    const headers = {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + randomPsp.Token
    }

    const url = 'http://localhost:5045/Payments';
    const response = http.post(url, body, { headers });
    if (response.status !== 201) {
        console.log(`Error creating payment: ${response.status} ${response.body}`);
        console.log(body);
        console.log(headers);
    }
}