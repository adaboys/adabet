<template>
  <v-app :style="style">
    <v-main>
      <animated-background />
      <message />
      <router-view />
    </v-main>

    <secondary-footer />
  </v-app>
</template>

<script>
import { config } from '~/plugins/config'
import Message from '~/components/Message'
import AnimatedBackground from '~/components/AnimatedBackground'
import SecondaryFooter from '~/components/SecondaryFooter'

export default {
  name: 'AuthLayout',

  components: { Message, AnimatedBackground, SecondaryFooter },

  computed: {
    appName () {
      return config('app.name')
    },
    style () {
      return {
        background: config('settings.theme.backgrounds.page'),
        '--body-font': config('settings.theme.fonts.body.family'),
        '--heading-font': config('settings.theme.fonts.heading.family')
      }
    }
  }
}
</script>
