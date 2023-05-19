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
        {{ $t('Web3 wallet is not detected.') }}
        {{ $t('If you are using a desktop computer, install the Metamask wallet browser extension.') }}
        {{ $t('If you are using a mobile phone, use the in-app browser.') }}
      </p>
    </v-alert>
  </v-form>
</template>

<script>
import Web3AuthMixin from '~/mixins/Web3Auth'
import Web3 from 'web3'

export default {
  mixins: [Web3AuthMixin],

  created () {
    if (Web3.givenProvider) {
      this.isInstalled = true

      if (Web3.givenProvider.selectedAddress) {
        this.connectWallet()
      }
    }
  },

  methods: {
    async connectWallet () {
      const web3 = new Web3(Web3.givenProvider)
      let addresses

      try {
        addresses = await web3.eth.requestAccounts()
        this.isConnected = !!addresses[0]
        this.form.address = addresses[0]
      } catch (err) {
        //
      }
    },
    async sign (nonce) {
      const web3 = new Web3(Web3.givenProvider)

      try {
        this.form.signature = await web3.eth.personal.sign(nonce, this.form.address)
        return true
      } catch (error) {
        return false
      }
    }
  }
}
</script>
