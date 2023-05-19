<template>
  <div>
    <v-tabs v-if="tabs.length > 1" centered>
      <v-tab v-for="(tab,i) in tabs" :key="i" :to="{ name: tab.route }">
        {{ tab.name }}
      </v-tab>
    </v-tabs>
    <router-view />
  </div>
</template>

<script>
import { mapGetters } from 'vuex'

export default {
  middleware: ['auth', 'verified', '2fa_passed', 'admin'],

  data () {
    return {
      activeTab: null
    }
  },

  computed: {
    ...mapGetters({ gameProvidersPackageIsEnabled: 'package-manager/gameProvidersIsEnabled' }),
    tabs () {
      const tabs = [
        { route: 'admin.bonuses.general', name: this.$t('General') }
      ]

      if (this.gameProvidersPackageIsEnabled) {
        tabs.push({ route: 'admin.bonuses.providers.index', name: this.$t('Game providers') })
      }

      return tabs
    }
  }
}
</script>
