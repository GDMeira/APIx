import http from 'k6/http';
import { SharedArray } from 'k6/data';

export const options = {
    scenarios: {
        contacts: {
            executor: 'constant-arrival-rate',
            duration: '1m',
            preAllocatedVUs: 100,
            rate: 30000,
            timeUnit: '1m'
        },
    }
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
        console.log(headers);
    }

    function generateRandomKey(user) {
        const types = ['Email', 'Phone', 'Random'];
        const type = types[Math.floor(Math.random() * types.length)];
        let value = '';

        if (type === 'Email') {
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