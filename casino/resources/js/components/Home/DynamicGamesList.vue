<template>
  <div v-if="games.length" :class="classes">
    <v-row>
      <v-col>
        <h3 class="text-h5 text-sm-h4 font-weight-thin text-center">
          {{ title }}
        </h3>
      </v-col>
    </v-row>

    <v-row v-if="filter && categories.length > 1">
      <v-col>
        <v-chip-group
          v-model="category"
          active-class="primary"
          mandatory
        >
          <v-chip label active @click="filterByCategory()">
            {{ $t('All') }}
          </v-chip>
          <v-chip v-for="cat in categories" :key="cat" label @click="filterByCategory(cat)">
            {{ cat }}
          </v-chip>
        </v-chip-group>
      </v-col>
    </v-row>

    <v-row v-if="search" justify="center">
      <v-col md="6" lg="3">
        <v-text-field
          v-model="input"
          :label="$t('Search')"
          solo
          clearable
          rounded
          dense
          prepend-inner-icon="mdi-magnify"
        />
      </v-col>
    </v-row>

    <v-row v-if="gamesList.length" ref="games" justify="center">
      <v-col
        v-for="(game, i) in gamesList"
        :key="i"
        class="game-card"
        :class="cardClasses"
        :data-name="game.name"
        :data-groups="JSON.stringify([game.category])"
      >
        <component :is="`game-${displayStyle}`" :game="game" class="mx-auto" :class="`display-style-${displayStyle}`" />
      </v-col>
    </v-row>
    <v-row v-else>
      <v-col v-for="(v,i) in Array(count).fill(0)" :key="i">
        <v-skeleton-loader type="card" />
      </v-col>
    </v-row>
    <v-row v-if="count > 0 && count < games.length" class="my-10">
      <v-col class="text-center">
        <v-btn color="primary" large @click="loadMore">
          <v-icon left>
            mdi-autorenew
          </v-icon>
          {{ $t('Load more') }}
        </v-btn>
      </v-col>
    </v-row>
  </div>
</template>

<script>
import GameCard from '~/components/Home/GameCard'
import GameBlock from '~/components/Home/GameBlock'
import GameBlock2 from '~/components/Home/GameBlock2'
import { unique } from '~/plugins/utils'
import Shuffle from 'shufflejs'
import { config } from '~/plugins/config'

export default {
  components: { GameBlock, GameCard, GameBlock2 },

  props: {
    title: {
      type: String,
      required: true
    },
    games: {
      type: Array,
      required: true
    },
    displayStyle: {
      type: String,
      required: true
    },
    displayCount: {
      type: Number,
      required: false,
      default: 0
    },
    loadCount: {
      type: Number,
      required: false,
      default: 0
    },
    filter: {
      type: Boolean,
      required: false,
      default: false
    },
    search: {
      type: Boolean,
      required: false,
      default: false
    }
  },

  data () {
    return {
      count: this.displayCount,
      category: null,
      shuffle: null,
      input: null
    }
  },

  computed: {
    classes () {
      return ['game-list', 'game-list-' + this.title.replace(/[^a-z]+/ig, '-').toLowerCase()]
    },
    cardClasses () {
      return config('settings.content.home.card_classes')
    },
    categories () {
      return unique(this.games.map(game => game.category)).sort()
    },
    gamesList () {
      return this.count > 0 ? this.games.slice(0, this.count) : this.games
    }
  },

  watch: {
    input (input) {
      this.filterByInput(input)
    }
  },

  methods: {
    initShuffle () {
      if (!this.shuffle) {
        this.shuffle = new Shuffle(this.$refs.games, { itemSelector: '.game-card' })
      }
    },
    filterByCategory (category = null) {
      this.initShuffle()

      if (category !== null) {
        this.shuffle.filter(category)
      } else {
        this.shuffle.filter()
      }
    },
    filterByInput (input = null) {

      this.initShuffle()

      if (input !== null && this.category) {
        this.category = null
      }

      if (input !== null) {
        this.shuffle.filter((element, shuffle) => {
          return element.dataset.name.toLowerCase().trim().includes(input.toLowerCase())
        })
      } else {
        this.shuffle.filter()
      }
    },
    loadMore () {
      this.count += this.loadCount
    }
  }
}
</script>
<style lang="scss" scoped>
.v-chip-group::v-deep {
  .v-slide-group__content {
    justify-content: center;
  }
}
</style>
