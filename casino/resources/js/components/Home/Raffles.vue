<template>
  <v-container v-if="rafflePackageIsEnabled" class="home-page-raffles mt-10">
    <v-row>
      <v-col class="text-center">
        <h3 class="text-h5 text-sm-h4 font-weight-thin">
          {{ $t('Buy tickets and win raffles') }}
        </h3>
      </v-col>
    </v-row>
    <v-row justify="center">
      <template v-if="raffles">
        <template v-if="raffles.length">
          <v-col
            v-for="raffle in raffles"
            :key="raffle.id"
            class="text-center mx-auto"
            :class="cardClasses"
          >
            <component :is="`raffle-${displayStyle}`" :raffle="raffle" :class="`display-style-${displayStyle}`" />
          </v-col>
        </template>
        <h5 v-else class="text-h5 mt-5">
          {{ $t('No raffles are running at the moment.') }}
        </h5>
      </template>
      <template v-else>
        <v-col v-for="i in [0,1,2]" :key="i" cols="12" md="4" lg="3">
          <v-skeleton-loader type="card" />
        </v-col>
      </template>
    </v-row>
  </v-container>
</template>

<script>
import axios from 'axios'
import { config } from '~/plugins/config'
import RaffleCard from '~/components/Home/RaffleCard'
import RaffleCard2 from '~/components/Home/RaffleCard2'
import RaffleCard3 from '~/components/Home/RaffleCard3'
import { mapGetters } from 'vuex'

export default {
  components: { RaffleCard, RaffleCard2, RaffleCard3 },

  data () {
    return {
      raffles: null
    }
  },

  computed: {
    ...mapGetters({
      rafflePackageIsEnabled: 'package-manager/raffleIsEnabled'
    }),
    displayStyle () {
      return config('settings.content.home.raffles.display_style')
    },
    cardClasses () {
      return config('settings.content.home.card_classes')
    }
  },

  created () {
    if (this.rafflePackageIsEnabled) {
      this.pullRaffles()
    }
  },

  methods: {
    async pullRaffles () {
      const { data } = await axios.get('/api/pub/raffles')

      this.raffles = data
    }
  }
}
</script>
