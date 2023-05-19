import store from '~/store'
import { get } from '~/plugins/utils'
import { config } from '~/plugins/config'

export default async (to, from, next) => {
  if (config('settings.users.email_verification') && !get(store, 'state.auth.user.email_verified_at')) {
    next({ name: 'verification.index' })
  } else {
    next()
  }
}
