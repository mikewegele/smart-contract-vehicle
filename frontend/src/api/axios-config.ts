import axios from 'axios';

const axiosInstance = axios.create({
    baseURL: 'http://localhost:5147',
});

export function api(config: any) {
    return axiosInstance.request(config);
}