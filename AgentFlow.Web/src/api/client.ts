import axios from 'axios';

export const apiClient = axios.create({
  // Dev: falls back to '/api', proxied to the local API by vite.config.ts.
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000,
});