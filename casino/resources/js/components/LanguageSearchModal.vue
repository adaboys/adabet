<template>
  <v-dialog
    v-model="modal"
    min-width="350"
    max-width="500"
    @click:outside="close"
  >
    <template #activator="{ on }">
      <slot :on="on" />
    </template>
    <v-card>
      <v-toolbar>
        <v-toolbar-title>
          {{ $t('Choose language') }}
        </v-toolbar-title>
        <v-spacer />
        <v-btn icon @click="close">
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </v-toolbar>
      <v-card-text>
        <v-autocomplete
          v-model="value"
          color="primary"
          :placeholder="$t('Search...')"
          :items="Object.values(locales)"
          item-text="title"
          item-value="code"
          :loading="!locales"
          :disabled="!locales"
          autofocus
          prepend-inner-icon="mdi-magnify"
          append-icon=""
          hide-no-data
          hide-details
          rounded
          outlined
          class="py-5"
        >
          <template #item="{ item }">
            <v-list-item-icon>
              <country-flag :country="item.flag || item.code" class="my-0" />
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title>
                <span class="subtitle-1">
                  {{ item.title }}
                </span>
              </v-list-item-title>
            </v-list-item-content>
          </template>
        </v-autocomplete>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script>
import CountryFlag from 'vue-country-flag'
import { mapState } from 'vuex'
import { loadMessages } from '~/plugins/i18n'

export default {
  components: { CountryFlag },

  data () {
    return {
      modal: false,
      value: null
    }
  },

  computed: {
    ...mapState('lang', [
      'locale',
      'locales'
    ])
  },

  watch: {
    value (locale) {
      if (locale) {
        this.setLocale(locale)
      }
    }
  },

  created () {
    this.value = this.locale
  },

  methods: {
    setLocale (locale) {
      if (this.$i18n.locale !== locale) {
        loadMessages(locale)
        this.$store.dispatch('lang/setLocale', { locale })
        this.close()
      }
    },
    close () {
      this.modal = false
    }
  }
}
</script>
