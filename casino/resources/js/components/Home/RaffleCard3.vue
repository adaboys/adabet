<template>
  <v-hover v-slot="{ hover }">
    <v-card :elevation="hover ? 12 : 2" height="250px">
      <router-link :to="{ name: 'raffles' }" class="text-decoration-none">
        <v-img
          :src="raffle.banner || ''"
          class="raffle-banner align-center text-center fill-height"
          :class="{ hover }"
        >
          <template #default>
            <v-card-title class="flex-column justify-space-around fill-height" :class="{ 'd-none': !hover }">
              <div>{{ raffle.title }}</div>
              <v-icon x-large>
                mdi-play-circle-outline
              </v-icon>
              <div class="text-uppercase">
                {{ $t('Win up to {0}', [integer(raffle.max_pot_size)]) }}
              </div>
            </v-card-title>
          </template>
          <template #placeholder>
            <v-skeleton-loader type="image" class="fill-height" />
          </template>
        </v-img>
      </router-link>
    </v-card>
  </v-hover>
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
<style lang="scss" scoped>
.raffle-banner {
  &.v-image {
    &::v-deep {
      .v-responsive__sizer {
        height: 100% !important;
        transition: all 0.5s;
        background: rgba(0, 0, 0, 0);
      }

      .v-responsive__content {
        height: 100%;
      }

      .v-skeleton-loader {
        .v-skeleton-loader__image {
          height: 100%;
        }
      }
    }

    &.hover {
      &::v-deep {
        .v-responsive__sizer {
          background: rgba(0, 0, 0, 0.8) !important;
        }
      }
    }
  }
}
</style>
