import http from 'k6/http';
import { SharedArray } from 'k6/data';

export const options = {
    vus: 10000, //virtual users
    duration: '15s'
}

const data = new SharedArray('users', () => JSON.parse(open("../seed/existing_users.json")));
const pspData = new SharedArray('PSPs', () => JSON.parse(open("../seed/existing_payment_providers.json")));

export default function () {
    const randomUser = data[Math.floor(Math.random() * data.length)];
    const randomPsp = pspData[Math.floor(Math.random() * pspData.length)];

    const body = JSON.stringify({
        key: generateRandomKey(randomUser),
        user: {
            cpf: randomUser.Cpf,
        },
        account: {
            number: `${Date.now() + randomUser.Cpf}`,
            agency: `${Date.now() + randomUser.Name}`,
        }
    })

    const headers = {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + randomPsp.Token
    }

    const url = 'http://localhost:5045/Keys';
    const response = http.post(url, body, { headers });
    if (response.status !== 201) {
        console.log(`Error creating key: ${response.status} ${response.body}`);
        console.log(body);
    }

    function generateRandomKey(user) {
        const types = ['CPF', 'Email', 'Phone', 'Random'];
        const type = types[Math.floor(Math.random() * types.length)];
        let value = '';

        if (type === 'CPF') {
            value = user.Cpf;
        } else if (type === 'Email') {
            value = `${Date.now() + user.Cpf}@gmail.com`;
        } else if (type === 'Phone') {
            value = generatePhone();
        } else {
            value = '';
        }

        return { type, value }
    }

    function generatePhone() {
        let phone = '+';
        for (let i = 0; i < 13; i++) {
            phone += Math.floor(Math.random() * 9) + 1;
        }

        return phone;
    }
}