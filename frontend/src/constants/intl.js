import { defineMessages } from 'react-intl'

export const commonMessages = defineMessages({
    signUp: 'Sign up',
    login: 'Log in',
    signOut: 'Sign out',
    logoutSuccessful: 'Logout successful!',
    aircraft: 'Aircraft',
    commander: 'Commander',
    wingman: 'Wingman',
    material: 'Material',
    ticket: 'Ticket',
    password: 'Password',
    update: 'Update',
    yes: 'Yes',
    no: 'No',
    ok: 'OK',
    cancel: 'Cancel',
    confirm: 'Confirm',
    noContent: 'There is no data yet !'
});

export const validationMessages = defineMessages({
    required: 'Required',
    minLength: 'The {field} minimum length {len} characters',
    phoneInvalid: 'That Phone number combination is invalid.',
    emailInvalid: 'That Email combination is invalid.',
    phoneOrEmainInvalid: 'That Phone number/Email combination is invalid.',
    phoneAlreadyExist: 'The phone number already exists.'
});

export const rankTypes = defineMessages({
    common: 'Common',
    rare: 'Rare',
    mythical: 'Mythical',
    immortal: 'Immortal',
    legendary: 'Legendary'
});

export const translateRankType = (intl, rankType) => {
    if(!intl || !rankType)
        return '';

    switch (rankType) {
        case 1:
            return intl.formatMessage(rankTypes.common);
        case 2:
            return intl.formatMessage(rankTypes.rare);
        case 3:
            return intl.formatMessage(rankTypes.mythical);
        case 4:
            return intl.formatMessage(rankTypes.immortal);
        case 5:
            return intl.formatMessage(rankTypes.legendary);
        default:
            return '';
    }
}
