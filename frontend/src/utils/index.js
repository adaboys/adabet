import moment from 'moment';
import { SPORT_DEFAULT_ID, matcheTypes, oddsNameMapping, paths } from '@constants';

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

export const getStartTime = (utcTime) => {
    let result = {};
    try {
        const date = moment.utc(utcTime).local();
        const time = date.format('HH:mm');
        let shortDate;
        if (Math.abs(moment().diff(date, 'days')) >= 2) {
            shortDate = date.format('ll').split(',')[0];
        }
        else {
            shortDate = date.calendar().split(' ')[0];
        }
        result = {
            date: shortDate,
            time
        };
    } catch (ex) {

    }
    return result;
}

export const resolveTeamImgPathById = (id) => {
    if (id) {
        return `https://assets.b365api.com/images/team/s/${id}.png`
    }
}

export const groupBy = (items, key) => items.reduce(
    (result, item) => ({
        ...result,
        [item[key]]: [
            ...(result[item[key]] || []),
            item,
        ],
    }),
    {},
);

export const converOddsNames = (name, sportType) => {
    return oddsNameMapping[sportType]?.[name] || name;
}

export const getInt = value => {
    try {
        return parseInt(value);
    }
    catch(err) {console.log(err);
        return 0;
    }
}

export const checkMatchEnded = match => {
    const time = match?.start_at ? moment.utc(match.start_at).local() : null;

    return !time || time <= moment();
}

export const generateSportUrl = (queryStr) => {
    const { matchType, id } = queryStr || { matchType: matcheTypes.TOP, id };
    return `${paths.sport.replace('[matchType]', matchType || matcheTypes.TOP)}?id=${id || SPORT_DEFAULT_ID}`;
}