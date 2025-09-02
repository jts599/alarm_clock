import axios from 'axios';

// Get API base URL from environment or use localhost as default
const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000';

export const fetchData = async (endpoint: string): Promise<any> => {
    try {
        const response = await axios.get(`${API_BASE_URL}/${endpoint}`);
        return response.data;
    } catch (error) {
        console.error('Error fetching data:', error);
        throw error;
    }
};

export const postData = async (endpoint: string, data: any): Promise<any> => {
    try {
        const response = await axios.post(`${API_BASE_URL}/${endpoint}`, data);
        return response.data;
    } catch (error) {
        console.error('Error posting data:', error);
        throw error;
    }
};