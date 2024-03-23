import http from 'k6/http';
import { SharedArray } from 'k6/data';

export const options = {
    scenarios: {
        contacts: {
            executor: 'constant-arrival-rate',
            duration: '1m',
            preAllocatedVUs: 100,
            rate: 40000,
            timeUnit: '1m'
        },
    }
}

const pixKeys = new SharedArray('pixKey', () => JSON.parse(open("../seed/existing_pix_keys.json")));
const pspData = new SharedArray('PSPs', () => JSON.parse(open("../seed/existing_payment_providers.json")));

export default function () {
    const randomPixKey = pixKeys[Math.floor(Math.random() * pixKeys.length)];
    const randomPsp = pspData[Math.floor(Math.random() * pspData.length)];

    const headers = {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + randomPsp.Token
    }

    const url = `http://localhost:5045/Keys/${randomPixKey.Type}/${randomPixKey.Value}`;
    const response = http.get(url, { headers });
    if (response.status !== 200) {
        console.log(`Error retrieving key: ${response.status}`);
        console.log(randomPixKey.Type);
        console.log(randomPixKey.Value);
    }
}