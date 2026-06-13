import axios from 'axios';
import { message } from 'antd';

/**
 * Настроенный экземпляр Axios для взаимодействия с Java-микросервисами.
 *
 * Конфигурация:
 * - baseURL: адрес API-шлюза (настраивается через .env)
 * - Интерцепторы: автоматическая подстановка JWT-токена (Keycloak)
 * - Обработка ошибок: 401 → редирект на логин, 5xx → уведомление
 */

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8081/api';

const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  },
});

/* ===== Request Interceptor: подставляем JWT-токен ===== */
axiosInstance.interceptors.request.use(
  (config) => {
    // TODO: Интеграция с Keycloak
    // const token = keycloak.token;
    const token = localStorage.getItem('auth_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

/* ===== Response Interceptor: обработка ошибок ===== */
axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    const { response } = error;

    if (!response) {
      // Сетевая ошибка
      message.error('Ошибка сети: сервер недоступен. Проверьте подключение.');
      return Promise.reject(error);
    }

    switch (response.status) {
      case 401:
        message.warning('Сессия истекла. Пожалуйста, войдите в систему заново.');
        // TODO: редирект на Keycloak login
        // keycloak.login();
        break;
      case 403:
        message.error('Недостаточно прав для выполнения операции.');
        break;
      case 422:
        // Ошибка валидации от сервера
        if (response.data?.errors) {
          const errorMessages = Object.values(response.data.errors).flat().join('; ');
          message.error(`Ошибка валидации: ${errorMessages}`);
        } else {
          message.error('Некорректные данные. Проверьте введённые значения.');
        }
        break;
      case 500:
      case 502:
      case 503:
        message.error('Ошибка сервера. Попробуйте позже.');
        break;
      default:
        message.error(`Ошибка: ${response.statusText || 'Неизвестная ошибка'}`);
    }

    return Promise.reject(error);
  }
);

export default axiosInstance;
