<template>
  <v-hover v-slot="{ hover }">
    <v-card :elevation="hover ? 12 : 2">
      <router-link :to="{ name: 'raffles' }" class="text-decoration-none">
        <v-img
          :src="raffle.banner || ''"
          height="250px"
          class="raffle-banner align-center text-center"
          :class="{ hover }"
        >
          <template #default>
            <v-card-title class="justify-center">
              <v-chip color="rgba(0, 0, 0, 0.4)" label x-large pill text-color="white" class="font-weight-thin text-uppercase">
                {{ raffle.title }}
              </v-chip>
            </v-card-title>
          </template>
          <template #placeholder>
            <v-skeleton-loader type="image" class="fill-height" />
          </template>
        </v-img>
      </router-link>
      <v-card-subtitle class="text-uppercase">
        {{ $t('Win up to {0}', [integer(raffle.max_pot_size)]) }}
      </v-card-subtitle>
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

      .v-skeleton-loader {
        .v-skeleton-loader__image {
          height: 100%;
        }
      }
    }

    &.hover {
      &::v-deep {
        .v-responsive__sizer {
          background: rgba(0, 0, 0, 0.55) !important;
        }
      }
    }

    .v-chip {
      cursor: pointer;
    }
  }
}
</style>
