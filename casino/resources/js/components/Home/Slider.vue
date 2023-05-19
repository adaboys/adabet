<template>
  <v-carousel
    :cycle="slider.cycle && slidesCount > 1"
    :hide-delimiters="!slider.pagination || slidesCount === 1"
    :show-arrows="slider.navigation && slidesCount > 1"
    :interval="slider.interval * 1000"
    hide-delimiter-background
    :height="isMobile ? (slider.height_mobile || 300) : slider.height"
    class="home-page-slider"
  >
    <v-carousel-item
      v-for="(slide, i) in slider.slides"
      :key="i"
    >
      <v-img :src="slide.image.url" class="slide-image align-center text-center fill-height">
        <template #default>
          <v-card-title class="fill-height pa-0">
            <v-row align="center" class="fill-height" :class="{ 'darken-60': slider.overlay }">
              <v-col class="text-center" cols="12">
                <h2 v-if="slide.title" class="text-h5 text-sm-h4 text-md-h3 text-lg-h2">
                  {{ slide.title }}
                </h2>
                <h3 v-if="slide.subtitle" class="text-h6 text-sm-h5 text-md-h4 text-lg-h3 font-weight-thin mt-5">
                  {{ slide.subtitle }}
                </h3>
                <div v-if="slide.link.title" class="mt-5">
                  <v-btn color="primary" large :href="slide.link.url">
                    {{ slide.link.title }}
                  </v-btn>
                </div>
              </v-col>
            </v-row>
          </v-card-title>
        </template>
      </v-img>
    </v-carousel-item>
  </v-carousel>
</template>

<script>
import { config } from '~/plugins/config'
import DeviceMixin from '~/mixins/Device'

export default {
  mixins: [DeviceMixin],

  computed: {
    slider () {
      return config('settings.content.home.slider')
    },
    slidesCount () {
      return this.slider ? this.slider.slides.length : 0
    }
  }
}
</script>
<style lang="scss" scoped>
.darken-60 {
  background: rgba(0, 0, 0, 0.6);
}
.slide-image::v-deep {
  .v-responsive__content {
    height: 100%;
  }
}
</style>
