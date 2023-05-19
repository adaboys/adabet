<script>
import Form from 'vform'
import axios from 'axios'
import { route } from '~/plugins/route'
import { ucfirst } from '~/plugins/utils'

export default {
  middleware: 'guest',

  metaInfo () {
    return { title: this.$t('{0} authentication', [ucfirst(this.$options._componentTag)]) }
  },

  data () {
    return {
      isLoading: false,
      isInstalled: false,
      isConnected: false,
      form: new Form({
        address: null,
        signature: null
      })
    }
  },

  methods: {
    async login () {
      this.isLoading = true
      const { data: { nonce } } = await axios.get(route('auth.web3.nonce'))

      if (await this.sign(nonce)) {
        const { data: { success, user, message } } = await this.form.post(route('auth.web3.login').replace('{provider}', this.$options._componentTag))

        if (success) {
          this.$store.dispatch('auth/updateUser', user)

          if (user.two_factor_auth_enabled && !user.two_factor_auth_passed) {
            this.$router.push({ name: '2fa' })
          } else {
            this.$router.push({ name: 'home' })
          }
        } else {
          this.$store.dispatch('message/error', { text: message })
        }
      }

      this.isLoading = false
    }
  }
}
</script>
