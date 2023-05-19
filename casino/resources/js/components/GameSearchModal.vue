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
          {{ $t('Choose a game') }}
        </v-toolbar-title>
        <v-spacer />
        <v-btn icon @click="close">
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </v-toolbar>
      <v-card-text>
        <v-autocomplete
          v-model="game"
          color="primary"
          :placeholder="$t('Search...')"
          :return-object="true"
          :items="games"
          :loading="!games"
          :disabled="!games"
          autofocus
          item-text="name"
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
              <v-img
                :src="item.banner"
                width="60px"
                class="game-banner align-center text-center"
              />
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title>
                <span class="subtitle-1">
                  {{ item.name }}
                </span>
              </v-list-item-title>
              <v-list-item-subtitle v-if="item.provider">
                {{ item.provider.name }}
              </v-list-item-subtitle>
            </v-list-item-content>
          </template>
        </v-autocomplete>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script>
import { mapGetters } from 'vuex'

export default {
  props: {
    active: {
      type: Boolean,
      required: false,
      default: false
    }
  },

  data () {
    return {
      modal: false,
      game: null
    }
  },

  computed: {
    ...mapGetters({
      games: 'package-manager/getGames'
    })
  },

  watch: {
    game (game) {
      if (game) {
        this.$router.push(game.route)
        this.modal = false
        this.$emit('change', game)
      }
    }
  },

  created () {
    if (this.active) {
      this.modal = true
    }
  },

  methods: {
    close () {
      this.modal = false
      this.$emit('close')
    }
  }
}
</script>
