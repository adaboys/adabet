import qs from 'query-string';

const sendRequest = async (options, params = {}) => {
    let fullPath = options.path;
    let fetchRequest;
    let infoRequest;
    const headers = { ...options.headers };

    if (options.isAuth) {
        headers.Authorization = `Bearer ${options.accessToken}`;
    }

    if (headers['Content-Type'] === 'multipart/form-data') {
        const formData = new FormData();
        for (let key of Object.keys(params)) {
            formData.append(key, params[key]);
        }

        delete headers['Content-Type'];

        infoRequest = {
            method: options.method,
            headers,
            body: formData,
        };
    }
    else {
        if (options.method === 'GET') {
            let hasDefaultQuery = false;
            if (options.params && Object.keys(options.params).length > 0) {
                hasDefaultQuery = true;
                const defaultQuery = qs.stringify(options.params);
                fullPath = `${fullPath}?${defaultQuery}`;
            }
            if (Object.keys(params).length > 0) {
                const queryString = qs.stringify(params);
                fullPath = hasDefaultQuery ? `${fullPath}&${queryString}` : `${fullPath}${fullPath.includes('?') ? '&' : '?'}${queryString}`;
            }

            infoRequest = {
                method: options.method,
                headers
            };
        }
        else {
            infoRequest = {
                method: options.method,
                headers,
                body: JSON.stringify(params)
            };
        }
    }

    // console.log("infoRequest Api:",infoRequest);
    fetchRequest = await fetch(fullPath, infoRequest)
        .catch(error => {
            console.log(error);
            return Promise.reject(error);
            // Redirect to error page
            // window.location.replace(errorPath);
        });

    if (fetchRequest.status === 401 && options.isAuth) {
        return { isLogout: true };
    }
    else if (fetchRequest.status === 403) {
        // window.location.replace('/forbidden');
    }
    else if (fetchRequest.status === 200 || fetchRequest.status === 201) {
        const responseData = await fetchRequest.json();
        return { success: true, responseData }
    }
    else if (fetchRequest.status === 401 || fetchRequest.status === 400 || fetchRequest.status === 404) {
        const responseData = await fetchRequest.json();
        return { success: false, responseData }
    }
    else {
        return Promise.reject(new Error('Internal Server Error'));
    }
}

const handleApiResponse = (result, onCompleted, onError) => {
    const { success, responseData } = result;
    if (success)
        onCompleted(responseData);
    else
        onError(responseData);
}

export { sendRequest, handleApiResponse }