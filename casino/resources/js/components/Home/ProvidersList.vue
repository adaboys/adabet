<template>
  <v-container v-if="packageEnabled && Object.keys(providers).length > 0" class="home-page-providers-list mt-10">
    <v-row>
      <v-col>
        <h3 class="text-h5 text-sm-h4 font-weight-thin text-center">
          {{ 'Game providers' }}
        </h3>
      </v-col>
    </v-row>
    <v-row justify="center" align="center">
      <v-col
        v-for="provider in providers"
        :key="provider.code"
        cols="6"
        lg="3"
      >
        <v-hover v-slot="{ hover }">
          <router-link :to="{ name: 'provider', params: { provider: provider.code } }">
            <v-img
              :src="provider.banner_url"
              :alt="provider.name"
              max-height="50px"
              :contain="true"
              max-width="200px"
              class="provider-banner mx-auto"
              :class="{ grayscale: !hover }"
            />
          </router-link>
        </v-hover>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import axios from 'axios'
import { route } from '~/plugins/route'
import { mapGetters } from 'vuex'

export default {
  data () {
    return {
      providers: []
    }
  },

  computed: {
    ...mapGetters({
      packageEnabled: 'package-manager/gameProvidersIsEnabled'
    })
  },

  async created () {
    if (this.packageEnabled) {
      const { data } = await axios.get(route('game-providers.providers.all'))
      this.providers = data
    }
  }
}
</script>
<style scoped lang="scss">
.provider-banner {
  transition: all ease 0.5s;

  &.grayscale {
    filter: grayscale(100%);
  }
}
</style>
