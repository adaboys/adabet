<template>
  <v-form @submit.prevent="login">
    <template v-if="isInstalled">
      <div v-if="isConnected" class="mb-5">
        <v-text-field
          :value="form.address"
          :label="$t('Your address')"
          :disabled="true"
          hide-details
          outlined
        />

        <v-row align="center" class="mt-3">
          <v-col class="text-center text-md-left">
            <v-btn type="submit" color="primary" :disabled="isLoading" :loading="isLoading">
              {{ $t('Log in') }}
            </v-btn>
          </v-col>
        </v-row>
      </div>
      <v-btn v-else color="secondary" class="mb-3" @click="connectWallet()">
        {{ $t('Connect wallet') }}
      </v-btn>
    </template>
    <v-alert
      v-else
      dense
      outlined
      text
      type="warning"
      class="justify-center align-center align-content-center"
    >
      <p>
        {{ $t('Tronlink wallet is not detected.') }}
        {{ $t('If you are using a desktop computer, install the Tronlink wallet browser extension.') }}
        {{ $t('If you are using a mobile phone, use the in-app browser.') }}
      </p>
    </v-alert>
  </v-form>
</template>

<script>
import Web3AuthMixin from '~/mixins/Web3Auth'

export default {
  mixins: [Web3AuthMixin],

  async created () {
    if (window.tronWeb) {
      this.isInstalled = true
      this.connectWallet()
    }
  },

  methods: {
    async connectWallet () {
      const address = window.tronWeb.defaultAddress.base58

      if (address !== false) {
        this.isConnected = true
        this.form.address = address
      }
    },
    async sign (nonce) {
      try {
        this.form.signature = await window.tronWeb.trx.sign(window.tronWeb.toHex(nonce).replace(/^0x/, ''))
        return true
      } catch (error) {
        return false
      }
    }
  }
}
</script>
