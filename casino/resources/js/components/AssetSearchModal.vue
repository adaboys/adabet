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
          {{ $t('Choose an asset') }}
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
          :placeholder="$t('Search') + '...'"
          :return-object="true"
          :items="items"
          :loading="searchInProgress"
          :search-input.sync="input"
          :disabled="disabled"
          clearable
          autofocus
          item-text="name"
          prepend-inner-icon="mdi-magnify"
          hide-no-data
          hide-details
          hide-selected
          rounded
          outlined
          class="py-5"
          @change="change"
        >
          <template #item="{ item }">
            <v-list-item-avatar
              color="primary"
              class="text-h5 font-weight-light justify-center"
            >
              {{ item.name.charAt(0).toUpperCase() }}
            </v-list-item-avatar>
            <v-list-item-content>
              <v-list-item-title v-text="item.name" />
              <v-list-item-subtitle v-text="item.symbol" />
            </v-list-item-content>
          </template>
        </v-autocomplete>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>
<script>
import { route } from '~/plugins/route'
import axios from 'axios'

export default {
  props: {
    type: {
      type: String,
      required: true
    },
    label: {
      type: String,
      required: false,
      default: ''
    },
    defaultAssetName: {
      type: String,
      required: true
    },
    disabled: {
      type: Boolean,
      required: false,
      default: false
    }
  },

  data () {
    return {
      modal: false,
      value: {},
      items: [],
      searchInProgress: false,
      input: null
    }
  },

  watch: {
    async input (value, previousValue) {
      if (value === null || this.searchInProgress) {
        return false
      }

      this.searchInProgress = true

      const { data: items } = await axios.post(route('assets.search'), { type: this.type, search: value, exact: previousValue === null })
      this.items = items

      if (previousValue === null && this.items.length) {
        this.$emit('change', this.items[0])
      }

      this.searchInProgress = false
    }
  },

  async created () {
    // trigger input change
    this.input = this.defaultAssetName
  },

  methods: {
    change (value) {
      if (value !== null) {
        this.$emit('change', value)
        this.modal = false
      }
    },
    close () {
      this.modal = false
    }
  }
}
</script>
