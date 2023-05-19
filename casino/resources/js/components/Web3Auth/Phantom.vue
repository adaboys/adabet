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
      <v-btn v-else color="secondary" class="mb-3" @click="connectWallet(false)">
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
        {{ $t('Phantom wallet is not detected.') }}
        {{ $t('If you are using a desktop computer, install the Phantom wallet browser extension.') }}
        {{ $t('If you are using a mobile phone, use the in-app browser.') }}
      </p>
    </v-alert>
  </v-form>
</template>

<script>
import Web3AuthMixin from '~/mixins/Web3Auth'
import bs58 from 'bs58'

export default {
  mixins: [Web3AuthMixin],

  created () {
    if (window.solana) {
      this.isInstalled = window.solana && window.solana.isPhantom
      this.connectWallet(true)
    }
  },

  methods: {
    async connectWallet (silent) {
      try {
        await window.solana.connect({ onlyIfTrusted: silent })
        this.isConnected = window.solana.isConnected
        this.form.address = window.solana.publicKey.toString()
      } catch (err) {
        //
      }
    },
    async sign (nonce) {
      try {
        const response = await window.solana.signMessage(new TextEncoder().encode(nonce), 'utf8')
        this.form.signature = bs58.encode(response.signature)
        return true
      } catch (error) {
        return false
      }
    }
  }
}
</script>
