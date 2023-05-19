<template>
  <v-footer
    :app="inset"
    :inset="inset"
    :color="background"
  >
    <language-search-modal>
      <template #default="{ on }">
        <v-btn
          icon
          small
          v-on="on"
        >
          <country-flag :country="flag" size="small" />
        </v-btn>
      </template>
    </language-search-modal>
    <game-search-modal v-if="authenticated">
      <template #default="{ on }">
        <v-btn
          class="ml-4"
          icon
          x-small
          color="secondary lighten-4"
          v-on="on"
        >
          <v-icon>mdi-play</v-icon>
        </v-btn>
      </template>
    </game-search-modal>
    <v-menu
      offset-y
      top
      right
      max-height="300"
    >
      <template #activator="{ on }">
        <v-btn
          class="ml-4 mr-2"
          icon
          x-small
          color="secondary lighten-4"
          v-on="on"
        >
          <v-icon>mdi-information-variant</v-icon>
        </v-btn>
      </template>

      <v-list>
        <template v-for="page in pages">
          <v-list-item :key="page.id" :to="{ name: 'page', params: { id: page.id } }" link exact>
            <v-list-item-content>
              <v-list-item-title>
                {{ page.title }}
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>
      </v-list>
    </v-menu>
    <v-spacer class="flex-grow-0 flex-sm-grow-1" />
    <template v-if="links.length">
      <v-hover v-for="(link, i) in links" :key="i" v-slot="{ hover }">
        <v-btn
          class="mx-2 my-4 my-lg-0"
          icon
          x-small
          :color="hover ? link.color : 'secondary lighten-4'"
          :href="link.url"
          target="_blank"
        >
          <v-icon>
            {{ link.icon }}
          </v-icon>
        </v-btn>
      </v-hover>
    </template>
  </v-footer>
</template>

<script>
import CountryFlag from 'vue-country-flag'
import { config } from '~/plugins/config'
import LanguageSearchModal from '~/components/LanguageSearchModal'
import GameSearchModal from '~/components/GameSearchModal'
import { mapGetters } from 'vuex'

export default {
  components: { CountryFlag, LanguageSearchModal, GameSearchModal },

  props: {
    inset: {
      type: Boolean,
      required: false,
      default: false
    }
  },

  data () {
    return {
      pages: config('settings.content.footer.menu')
    }
  },

  computed: {
    ...mapGetters({
      authenticated: 'auth/check',
      flag: 'lang/flag'
    }),
    background () {
      return config('settings.theme.backgrounds.footer')
    },
    links () {
      return config('settings.content.social')
    }
  }
}
</script>
