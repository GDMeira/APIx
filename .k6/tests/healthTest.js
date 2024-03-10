import http from 'k6/http';

export const options = {
    vus: 100, //virtual users
    duration: '10s'   
}

export default function () {
    http.get('http://localhost:5045/health');
}