import moment from 'moment';

export const isEmptyObject = (obj) => {
    return obj && Object.keys(obj).length === 0 && obj.constructor === Object;
}

export const cleanObject = (obj) => {
    let result = {};
    if (obj) {
        Object.keys(obj).forEach((key) => {
            if ((!Array.isArray(obj[key]) && obj[key]) || obj[key]?.length)
                result[key] = obj[key];
        });
    }
    return result;
};

export const getElapsedSecondsText = (seconds) => {
    return moment().subtract('seconds', seconds || 0).fromNow();
};

export const paddingText = (width, str, padding) => {
    return (width <= str?.length) ? str : paddingText(width, padding + str, padding);
}

export const formatNumber = (value) => {
    if (value) {
        const decimalPosition = value.toString().indexOf(".");
        if (decimalPosition > 0) {
            const intVal = value.toString().substring(0, decimalPosition);
            const decimalVal = value.toString().substring(decimalPosition + 1);
            return `${intVal.replace(/\B(?=(\d{3})+(?!\d))/g, ",")}.${decimalVal}`;
        }
        return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
    }
    return "";
};


export const isMobileOnUserAgent = (userAgent) => {
    if (!userAgent) {
        return false;
    }
    const toMatch = [
        /Android/i,
        /webOS/i,
        /iPhone/i,
        /iPad/i,
        /iPod/i,
        /BlackBerry/i,
        /Windows Phone/i
    ];
    return toMatch.some((toMatchItem) => {
        return userAgent.match(toMatchItem);
    });
}

export const getStartTime = (dateStr) => {
    let result = {};
    try {
        const date = moment(dateStr);
        const time = date.format('HH:mm');
        let shortDate;
        if(Math.abs(moment().diff(date, 'days')) >= 2) {
            shortDate = date.format('ll').split(',')[0];
        }
        else {
            shortDate = date.calendar().split(' ')[0];
        }
        result = {
            date: shortDate,
            time
        };
    } catch(ex) {

    }
    return result;
}