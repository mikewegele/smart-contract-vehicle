import axios from 'axios';

const axiosInstance = axios.create({
    baseURL: 'http://localhost:5147',
});

export function api(config) {
    return axiosInstance.request(config);
}
