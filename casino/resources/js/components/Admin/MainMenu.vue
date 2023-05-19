<template>
  <v-list dense>
    <template v-for="item in menu">
      <v-list-item v-if="item.enabled && canAccess(item.route)" :key="item.route" :to="{ name: item.route }" link exact>
        <v-list-item-action>
          <v-icon>{{ item.icon }}</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title>{{ item.title }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
    </template>
  </v-list>
</template>

<script>
import { mapGetters, mapState } from 'vuex'

export default {
  computed: {
    ...mapState('auth', ['user']),
    ...mapGetters({
      rafflePackageIsEnabled: 'package-manager/raffleIsEnabled',
      paymentsPackageIsEnabled: 'package-manager/paymentsIsEnabled'
    }),
    menu () {
      return [
        { route: 'admin.dashboard.index', icon: 'mdi-view-dashboard', title: this.$t('Dashboard'), enabled: true },
        { route: 'admin.users.index', icon: 'mdi-account-multiple', title: this.$t('Users'), enabled: true },
        { route: 'admin.accounts.index', icon: 'mdi-account-cash', title: this.$t('Accounts'), enabled: true },
        { route: 'admin.bonuses.index', icon: 'mdi-gift', title: this.$t('Bonuses'), enabled: true },
        { route: 'admin.deposits.index', icon: 'mdi-cash-plus', title: this.$t('Deposits'), enabled: this.paymentsPackageIsEnabled },
        { route: 'admin.withdrawals.index', icon: 'mdi-cash-minus', title: this.$t('Withdrawals'), enabled: this.paymentsPackageIsEnabled },
        { route: 'admin.affiliate.index', icon: 'mdi-link', title: this.$t('Affiliates'), enabled: true },
        { route: 'admin.games.index', icon: 'mdi-cards-playing-outline', title: this.$t('Games'), enabled: true },
        { route: 'admin.raffles.index', icon: 'mdi-ticket', title: this.$t('Raffles'), enabled: this.rafflePackageIsEnabled },
        { route: 'admin.chat.index', icon: 'mdi-message-outline', title: this.$t('Chat'), enabled: true },
        { route: 'admin.deposit-methods.index', icon: 'mdi-cash-marker', title: this.$t('Deposit methods'), enabled: this.paymentsPackageIsEnabled },
        { route: 'admin.withdrawal-methods.index', icon: 'mdi-cash-marker', title: this.$t('Withdrawal methods'), enabled: this.paymentsPackageIsEnabled },
        { route: 'admin.settings.index', icon: 'mdi-cog', title: this.$t('Settings'), enabled: true },
        { route: 'admin.maintenance.index', icon: 'mdi-server', title: this.$t('Maintenance'), enabled: true },
        { route: 'admin.add-ons.index', icon: 'mdi-shape', title: this.$t('Add-ons'), enabled: true },
        { route: 'admin.license.index', icon: 'mdi-license', title: this.$t('License'), enabled: true },
        { route: 'admin.help.index', icon: 'mdi-help', title: this.$t('Help'), enabled: true }
      ]
    }
  },

  methods: {
    canAccess (route) {
      const [module, ...rest] = route.replace('admin.', '').split('.')
      return this.user.permissions === null || this.user.permissions[module] >= 1
    }
  }
}
</script>
