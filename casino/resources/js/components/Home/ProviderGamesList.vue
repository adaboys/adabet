<template>
  <v-container v-if="packageEnabled" class="home-page-provider-games">
    <dynamic-games-list
      v-for="(item, i) in featuredCategoryGames"
      :key="i"
      :title="item.title"
      :games="item.games"
      :display-style="config('settings.content.home.provider_games.display_style')"
      :display-count="config('settings.content.home.provider_games.display_count')"
      :load-count="config('settings.content.home.provider_games.load_count')"
      class="mt-10"
    />
  </v-container>
</template>

<script>
import { config } from '~/plugins/config'
import DynamicGamesList from '~/components/Home/DynamicGamesList'
import { mapGetters } from 'vuex'
import cloneDeep from 'clone-deep'

export default {
  components: { DynamicGamesList },

  data () {
    return {
      featuredCategories: config('settings.content.home.provider_games.featured_categories')
    }
  },

  computed: {
    ...mapGetters({
      packageEnabled: 'package-manager/gameProvidersIsEnabled',
      getProviderGamesByCategpries: 'package-manager/getProviderGamesByCategpries'
    }),
    featuredCategoryGames () {
      return cloneDeep(this.featuredCategories)
        .map(item => {
          item.games = this.getProviderGamesByCategpries(item.categories)
          return item
        })
    }
  },

  methods: {
    config
  }
}
</script>
