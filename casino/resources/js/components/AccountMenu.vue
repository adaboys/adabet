<template>
  <v-menu v-model="menu" offset-y left>
    <template #activator="{ on }">
      <v-btn
        v-show="!isProviderGamePage"
        icon
        rounded
        class="px-2 width-auto"
        v-on="on"
      >
        <v-icon class="mr-1">
          {{ creditsIcon }}
        </v-icon>
        <animated-number v-if="account" :number="account.balance" />
      </v-btn>
    </template>
    <v-list>
      <v-subheader class="text-uppercase">
        {{ $t('Account') }}
      </v-subheader>
      <v-list-item :to="{ name: 'user.account.transactions' }" link exact>
        <v-list-item-icon>
          <v-icon>mdi-format-list-bulleted</v-icon>
        </v-list-item-icon>
        <v-list-item-content>
          <v-list-item-title>{{ $t('Transactions') }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-list-item v-if="paymentsPackageIsEnabled" :to="{ name: 'user.account.deposits.index' }" link exact>
        <v-list-item-icon>
          <v-icon>mdi-cash-plus</v-icon>
        </v-list-item-icon>
        <v-list-item-content>
          <v-list-item-title>{{ $t('Deposits') }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-list-item v-if="paymentsPackageIsEnabled" :to="{ name: 'user.account.withdrawals.index' }" link exact>
        <v-list-item-icon>
          <v-icon>mdi-cash-minus</v-icon>
        </v-list-item-icon>
        <v-list-item-content>
          <v-list-item-title>{{ $t('Withdrawals') }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-list-item v-if="faucetEnabled" :to="{ name: 'user.account.faucet' }" link exact>
        <v-list-item-icon>
          <v-icon>mdi-chip</v-icon>
        </v-list-item-icon>
        <v-list-item-content>
          <v-list-item-title>{{ $t('Faucet') }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-list-item :to="{ name: 'user.account.deposit-internal' }" link exact>
        <v-list-item-icon>
          <v-icon>mdi-chip</v-icon>
        </v-list-item-icon>
        <v-list-item-content>
          <v-list-item-title>{{ $t('Deposit Internal') }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
    </v-list>
  </v-menu>
</template>

<script>
import AnimatedNumber from '~/components/AnimatedNumber'
import { mapGetters, mapState } from 'vuex'
import { config } from '~/plugins/config'

export default {
  components: {
    AnimatedNumber
  },

  data () {
    return {
      menu: false
    }
  },

  computed: {
    ...mapState('auth', ['account']),
    ...mapGetters({
      paymentsPackageIsEnabled: 'package-manager/paymentsIsEnabled'
    }),
    creditsIcon () {
      return config('settings.interface.credits_icon')
    },
    faucetEnabled () {
      return config('settings.bonuses.faucet.amount') > 0
    },
    isProviderGamePage () {
      return this.$route.name === 'provider.game'
    }
  }
}
</script>
