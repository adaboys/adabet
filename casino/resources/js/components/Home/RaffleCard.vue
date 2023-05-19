<template>
  <v-card outlined raised color="primary">
    <v-img
      :src="raffle.banner || ''"
      class="white--text align-center"
      gradient="to bottom, rgba(0,0,0,0.8), rgba(0,0,0,0.8)"
      height="200px"
    >
      <v-card-title class="d-block text-center">
        {{ raffle.title }}
      </v-card-title>
    </v-img>
    <v-card-text>
      <v-btn :to="{ name: 'raffles' }" outlined depressed>
        {{ $t('Win up to {0}', [integer(raffle.max_pot_size)]) }}
      </v-btn>
    </v-card-text>
  </v-card>
</template>
<script>
import { integer } from '~/plugins/format'

export default {
  props: {
    raffle: {
      type: Object,
      required: true
    }
  },

  methods: {
    integer
  }
}
</script>
