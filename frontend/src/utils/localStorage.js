import { ssrMode } from "@constants";

export const setObjectData = (key, data) => {
    if (typeof window !== 'undefined' && window.localStorage) {
        window.localStorage.setItem(key, JSON.stringify(data));
    }
}

export const getObjectData = (key) => {
    if (typeof window !== 'undefined' && window.localStorage) {
        const jsonData = window.localStorage.getItem(key);
        if (jsonData) {
            try {
                return JSON.parse(jsonData);
            }
            catch {
                return false;
            }
        }
        return false;
    }
    return false;
}

export const setStringData = (key, value) => {
    if (typeof window !== 'undefined' && window.localStorage) {
        window.localStorage.setItem(key, value);
    }
}

export const getStringData = (key) => {
    if (typeof window !== 'undefined' && window.localStorage) {
        return window.localStorage.getItem(key);
    }
    return false;
}

export const removeItem = (key) => {
    if (typeof window !== 'undefined' && window.localStorage) {
        window.localStorage.removeItem(key);
    }
}
// ms -> milliseconds
export const setCookie = (name, value, ms) => {
    let expires = '';
    if (ms) {
        const date = new Date();
        date.setTime(date.getTime() + ms);// ms = (days * 24 * 60 * 60 * 1000)
        expires = `; expires=${date.toUTCString()}`;
    }
    document.cookie = `${name}=${(value || '')}${expires}; path=/`;
}

export const getCookie = (cookieName, cookieStr) => {
    let cookieValue = null;
    const cookies = (cookieStr ? cookieStr.split(';') : []) || (!ssrMode ? document.cookie.split(';') : []);
    cookies.forEach((cookie) => {
        let i = cookie.indexOf('=');
        let name = cookie.substr(0, i).trim();
        let value = cookie.substr(i + 1).trim();
        if (name == cookieName) {
            cookieValue = value;
            return false;
        }
    })
    return cookieValue;
}

export const eraseCookie = (name) => {
    document.cookie = document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:01 GMT; path=/`;
}