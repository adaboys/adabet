<template>
  <v-list dense>
    <v-list-item :to="{ name: 'home' }" link exact>
      <v-list-item-action>
        <v-icon>mdi-home</v-icon>
      </v-list-item-action>
      <v-list-item-content>
        <v-list-item-title>{{ $t('Home') }}</v-list-item-title>
      </v-list-item-content>
    </v-list-item>
    <template v-if="authenticated">
      <v-list-group
        v-if="originalGames.length"
        prepend-icon="mdi-fire"
        no-action
        class="main-menu-original-games"
      >
        <template #activator>
          <v-list-item-title>{{ $t('Original games') }}</v-list-item-title>
        </template>
        <template v-for="(game, i) in originalGames">
          <v-list-item :key="i" :to="game.route" link exact>
            <v-list-item-content>
              <v-list-item-title>
                {{ game.name }}
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>
      </v-list-group>
      <template v-if="gameProvidersPackageIsEnabled">
        <v-list-group
          v-for="(category, i) in featuredCategoryGames"
          :key="i"
          :prepend-icon="category.icon || 'mdi-cards-playing-outline'"
          no-action
          class="main-menu-provider-games"
        >
          <template #activator>
            <v-list-item-title>{{ category.title }}</v-list-item-title>
          </template>
          <template v-for="(game, k) in category.games">
            <v-list-item :key="k" :to="game.route" link exact>
              <v-list-item-content>
                <v-list-item-title>
                  {{ game.name }}
                </v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </template>
        </v-list-group>
      </template>
      <v-list-group
        v-if="Object.keys(predictions).length"
        prepend-icon="mdi-trending-up"
        no-action
        class="main-menu-predictions"
      >
        <template #activator>
          <v-list-item-title>{{ $t('Markets') }}</v-list-item-title>
        </template>
        <template v-for="(pkg, id) in predictions">
          <v-list-item :key="id" :to="{ name: 'prediction', params: { packageId: id }}" link exact>
            <v-list-item-content>
              <v-list-item-title>{{ pkg.name }}</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>
      </v-list-group>
      <v-list-item v-if="rafflePackageIsEnabled" :to="{ name: 'raffles' }" link exact class="main-menu-raffles">
        <v-list-item-action>
          <v-icon>mdi-ticket</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title>{{ $t('Raffles') }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-list-item v-if="paymentsPackageIsEnabled" :to="{ name: 'user.account.deposits.index' }" link exact class="main-menu-deposits">
        <v-list-item-action>
          <v-icon>mdi-cash-plus</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title>{{ $t('Deposits') }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-list-item v-if="paymentsPackageIsEnabled" :to="{ name: 'user.account.withdrawals.index' }" link exact class="main-menu-withdrawals">
        <v-list-item-action>
          <v-icon>mdi-cash-minus</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title>{{ $t('Withdrawals') }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-list-item :to="{ name: 'history' }" link exact class="main-menu-history">
        <v-list-item-action>
          <v-icon>mdi-history</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title>{{ $t('History') }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-list-item v-if="leaderboardPageEnabled" :to="{ name: 'leaderboard' }" link exact class="main-menu-leaderboard">
        <v-list-item-action>
          <v-icon>mdi-star</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title>{{ $t('Leaderboard') }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
    </template>
  </v-list>
</template>

<script>
import { mapGetters } from 'vuex'
import { config } from '~/plugins/config'
import cloneDeep from 'clone-deep'

export default {
  data () {
    return {
      featuredCategories: config('settings.content.home.provider_games.featured_categories')
    }
  },

  computed: {
    ...mapGetters({
      authenticated: 'auth/check',
      originalGames: 'package-manager/getOriginalGames',
      getProviderGamesByCategpries: 'package-manager/getProviderGamesByCategpries',
      gameProvidersPackageIsEnabled: 'package-manager/gameProvidersIsEnabled',
      paymentsPackageIsEnabled: 'package-manager/paymentsIsEnabled',
      rafflePackageIsEnabled: 'package-manager/raffleIsEnabled'
    }),
    featuredCategoryGames () {
      return cloneDeep(this.featuredCategories)
        .map(item => {
          item.games = this.getProviderGamesByCategpries(item.categories)
          return item
        })
    },
    leaderboardPageEnabled () {
      return config('settings.content.leaderboard.enabled')
    },
    predictions () {
      return this.$store.getters['package-manager/getByType']('prediction')
    }
  }
}
</script>
