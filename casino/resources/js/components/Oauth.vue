<template>
  <v-row v-if="providersCount" class="my-0">
    <v-col class="text-center">
      <v-btn v-for="(provider, id) in providers" :key="id" fab icon elevation="5" class="mx-2" @click="loginWith(id)">
        <v-tooltip bottom>
          <template #activator="{ on }">
            <v-icon large v-on="on">
              mdi-{{ provider.mdi || id }}
            </v-icon>
          </template>
          <span>{{ $t('Log in with {0}', [ucfirst(id)]) }}</span>
        </v-tooltip>
      </v-btn>
    </v-col>
  </v-row>
</template>

<script>
import { config } from '~/plugins/config'
import { ucfirst } from '~/plugins/utils'
import { mapState } from 'vuex'
import axios from 'axios'

export default {
  name: 'OAuth',

  computed: {
    ...mapState('auth', [
      'user'
    ]),
    providers () {
      return config('oauth')
    },
    providersCount () {
      return this.providers ? Object.keys(this.providers).length : 0
    }
  },

  methods: {
    ucfirst,
    async loginWith (provider) {
      const { data } = await axios.post(`/api/oauth/${provider}/url`)
      window.location.href = data.url
    }
  }
}
</script>
