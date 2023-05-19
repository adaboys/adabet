import Cookies from 'js-cookie'
import * as types from '../mutation-types'

// we can't use config store since it's not yet initialized, so using global window variable instead
const { locale, locales } = window.config.app

// state
export const state = {
  locale: Cookies.get('locale') || locale,
  locales
}

// getters
export const getters = {
  flag: state => state.locales[state.locale].flag || state.locales[state.locale].code
}

// mutations
export const mutations = {
  [types.SET_LOCALE] (state, { locale }) {
    state.locale = locale
  }
}

// actions
export const actions = {
  setLocale ({ commit }, { locale }) {
    commit(types.SET_LOCALE, { locale })
    Cookies.set('locale', locale, { expires: 365 })
  }
}
