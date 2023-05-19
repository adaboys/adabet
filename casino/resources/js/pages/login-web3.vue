<template>
  <v-container class="fill-height" fluid>
    <v-row align="center" justify="center">
      <v-col cols="12" sm="8" md="6" lg="4">
        <v-card class="elevation-12">
          <v-toolbar color="primary">
            <router-link :to="{ name: 'home' }">
              <v-avatar size="40" tile>
                <v-img :src="appLogoUrl" />
              </v-avatar>
            </router-link>
            <v-toolbar-title class="ml-2">
              {{ $t('{0} authentication', [componentName]) }}
            </v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <component :is="provider" v-if="componentExists" class="mt-5" />
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import Metamask from '~/components/Web3Auth/Metamask'
import Phantom from '~/components/Web3Auth/Phantom'
import Tronlink from '~/components/Web3Auth/Tronlink'
import { config } from '~/plugins/config'
import { ucfirst } from '~/plugins/utils'

export default {
  components: { Metamask, Phantom, Tronlink },

  middleware: 'guest',

  props: {
    provider: {
      type: String,
      required: true
    }
  },

  metaInfo () {
    return { title: this.$t('Web3 authentication') }
  },

  computed: {
    appLogoUrl () {
      return config('app.logo')
    },
    componentName () {
      return ucfirst(this.provider)
    },
    componentExists () {
      return typeof this.$options.components[this.componentName] !== 'undefined'
    }
  },

  beforeMount () {
    if (!this.componentExists || !config(`auth.web3.${this.provider}.enabled`)) {
      this.$router.replace({ name: 'login' })
    }
  }
}
</script>
