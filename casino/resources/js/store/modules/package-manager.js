import { UPDATE_ORIGINAL_GAMES, UPDATE_PROVIDER_GAMES } from '../mutation-types'
import axios from 'axios'

// state
export const state = {
  packages: window.packages || {},
  originalGames: [],
  providerGames: []
}

// getters
export const getters = {
  getById: state => id => state.packages[id] || null,
  getByType: state => type => {
    return Object.keys(state.packages)
      .filter(id => typeof type === 'string' ? state.packages[id].type === type : type.indexOf(state.packages[id].type) > -1)
      .reduce((a, id) => ({ ...a, [id]: state.packages[id] }), {})
  },
  gameProvidersIsEnabled: (state, getters) => !!getters.getById('game-providers'),
  raffleIsEnabled: (state, getters) => !!getters.getById('raffle'),
  paymentsIsEnabled: (state, getters) => !!getters.getById('payments'),
  predictionsIsEnabled: (state, getters) => !!getters.getById('prediction'),
  getOriginalGames: state => state.originalGames,
  getProviderGames: state => state.providerGames,
  getProviderGamesById: state => id => state.providerGames.filter(game => game.provider.id === id),
  getProviderGamesByCategpries: state => categories => {
    return state.providerGames
      .filter(game => categories.indexOf(game.category) > -1 || categories.indexOf(game.id) > -1)
      .sort((a, b) => {
        return a.name < b.name ? -1 : (a.name > b.name ? 1 : 0)
      })
  },
  getGames: (state, getters) => [...getters.getOriginalGames, ...getters.getProviderGames]
}

// mutations
export const mutations = {
  [UPDATE_ORIGINAL_GAMES] (state, games) {
    state.originalGames = games
  },
  [UPDATE_PROVIDER_GAMES] (state, games) {
    state.providerGames = games
  }
}

// actions
export const actions = {
  fetchOriginalGames ({ state, getters, rootGetters, commit }) {
    if (state.originalGames.length === 0) {
      const games = []
      const packages = getters.getByType('game')
      const config = rootGetters['config/get']

      Object.keys(packages).forEach(id => {
        if (config(id + '.variations')) {
          config(id + '.variations').forEach(variation => {
            games.push({
              id,
              slug: variation.slug,
              name: variation._title || variation.title,
              banner: variation.banner,
              categories: variation.categories || {},
              route: { name: 'game', params: { game: id, slug: variation.slug } }
            })
          })
        } else {
          games.push({
            id,
            name: packages[id].name,
            banner: config(`${id}.banner`),
            categories: config(`${id}.categories`) || [],
            route: { name: 'game', params: { game: id } }
          })
        }
      })

      commit(UPDATE_ORIGINAL_GAMES, games.sort((a, b) => {
        return a.name < b.name ? -1 : (a.name > b.name ? 1 : 0)
      }))
    }
  },
  async fetchProviderGames ({ state, getters, rootGetters, commit }) {
    const packageEnabled = getters.gameProvidersIsEnabled
    if (packageEnabled && state.providerGames.length === 0) {
      const route = rootGetters['route/get']
      const { data } = await axios.get(route('game-providers.games.all'))
      commit(UPDATE_PROVIDER_GAMES, data.map(game => ({ ...game, ...{ route: { name: 'provider.game', params: { provider: game.provider.id, game: game.id } } } })))
    }
  }
}
