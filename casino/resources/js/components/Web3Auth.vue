<template>
  <v-row v-if="providersCount" class="mb-3">
    <v-col v-for="(provider, id) in providers" :key="id" cols="12" class="text-center">
      <v-btn
        color="secondary"
        elevation="5"
        :to="{ name: 'login.web3', params: { provider: id } }"
      >
        {{ $t('Log in with {0}', [ucfirst(id)]) }}
      </v-btn>
    </v-col>
  </v-row>
</template>

<script>
import { config } from '~/plugins/config'
import { ucfirst } from '~/plugins/utils'
import { mapState } from 'vuex'

export default {
  computed: {
    ...mapState('auth', [
      'user'
    ]),
    providers () {
      return config('auth.web3')
    },
    providersCount () {
      return this.providers ? Object.keys(this.providers).length : 0
    }
  },

  methods: {
    ucfirst
  }
}
</script>
