<template>
  <v-app :class="navbarVisible ? 'permanent-navbar' : 'temporary-navbar'" :style="style">
    <system-bar v-if="!(isMobile && (isGamePage || isPredictionPage || isProviderGamePage)) && systemBarEnabled && authenticated" />

    <v-navigation-drawer
      v-model="navigationDrawer"
      app
      :permanent="navbarVisible"
      :temporary="!navbarVisible"
      :color="navBarBackground"
    >
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title class="title">
            {{ $t('Navigation') }}
          </v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-divider />
      <template v-if="user && user.is_admin">
        <admin-main-menu />
        <v-divider />
      </template>
      <main-menu />
    </v-navigation-drawer>

    <chat v-if="authenticated && chatEnabled" v-model="chatDrawer" @message="setUnreadChatMessagesCount" />

    <template v-if="isMobile && (isGamePage || isPredictionPage || isProviderGamePage)">
      <v-speed-dial fixed top left>
        <template #activator>
          <v-btn small outlined icon @click.stop="navigationDrawer = !navigationDrawer">
            <v-icon small>
              mdi-menu
            </v-icon>
          </v-btn>
        </template>
      </v-speed-dial>
      <v-speed-dial fixed top right>
        <template #activator>
          <v-btn
            v-show="!isProviderGamePage"
            small
            outlined
            :to="{ name: 'user.account.transactions' }"
            exact
          >
            <v-icon small>
              {{ creditsIcon }}
            </v-icon>
            <animated-number v-if="account" :number="account.balance" />
          </v-btn>
        </template>
      </v-speed-dial>
    </template>
    <v-app-bar v-else app :clipped-left="!navbarVisible" :color="appBarBackground">
      <v-app-bar-nav-icon v-if="!navbarVisible" @click.stop="navigationDrawer = !navigationDrawer" />

      <v-toolbar-title class="header-logo d-flex align-center">
        <router-link :to="{ name: 'home' }">
          <v-avatar size="40" tile>
            <v-img :src="appLogoUrl" :alt="appName" />
          </v-avatar>
        </router-link>
        <div class="ml-3 d-none d-sm-block text-h5">
          {{ appName }}
        </div>
      </v-toolbar-title>

      <v-spacer />

      <template v-if="!token && !authenticated">
        <v-btn :to="{ name: 'login' }" class="secondary">
          {{ $t('Log in') }}
        </v-btn>
        <v-btn @click="linkToAdabet" class="primary ml-2">
          {{ $t('Sign up') }}
        </v-btn>
      </template>
      <template v-else-if="authenticated">
        <account-menu />
        <settings-menu />
        <user-menu />

        <v-btn v-if="chatEnabled" icon @click="chatDrawer = !chatDrawer">
          <v-badge :content="unreadChatMessagesCount" :value="unreadChatMessagesCount" overlap>
            <v-icon>{{ chatDrawer ? 'mdi-message' : 'mdi-message-outline' }}</v-icon>
          </v-badge>
        </v-btn>
      </template>
      <preloader :active="loading" />
    </v-app-bar>

    <v-main>
      <message />
      <router-view id="content" />
    </v-main>

    <component :is="footerComponent" v-if="!(isMobile && (isGamePage || isPredictionPage || isProviderGamePage))" :inset="navbarVisible" />
  </v-app>
</template>

<script>
import { config } from '~/plugins/config'
import { mapState, mapGetters } from 'vuex'
import DeviceMixin from '~/mixins/Device'
import Message from '~/components/Message'
import Chat from '~/components/Chat'
import Preloader from '~/components/Preloader'
import SecondaryFooter from '~/components/SecondaryFooter'
import AdminFooter from '~/components/Admin/Footer'
import AnimatedNumber from '~/components/AnimatedNumber'
import SystemBar from '~/components/SystemBar'
import AdminMainMenu from '~/components/Admin/MainMenu'
import MainMenu from '~/components/MainMenu'
import AccountMenu from '~/components/AccountMenu'
import SettingsMenu from '~/components/SettingsMenu'
import UserMenu from '~/components/UserMenu'

export default {
  name: 'DefaultLayout',

  components: { UserMenu, SettingsMenu, AccountMenu, MainMenu, AdminMainMenu, SystemBar, Message, Chat, Preloader, SecondaryFooter, AdminFooter, AnimatedNumber },

  mixins: [DeviceMixin],

  data () {
    return {
      navigationDrawer: this.navbarVisible,
      chatDrawer: false,
      unreadChatMessagesCount: 0
    }
  },

  computed: {
    ...mapState('auth', ['user', 'account', 'token']),
    ...mapState('progress', ['loading']),
    ...mapGetters({
      authenticated: 'auth/check'
    }),
    appName () {
      return config('app.name')
    },
    appLogoUrl () {
      return config('app.logo')
    },
    appBarBackground () {
      return config('settings.theme.backgrounds.app_bar')
    },
    navBarBackground () {
      return config('settings.theme.backgrounds.nav_bar')
    },
    creditsIcon () {
      return config('settings.interface.credits_icon')
    },
    navbarVisible () {
      return config('settings.interface.navbar.visible') && !this.isMobile
    },
    isGamePage () {
      return this.$route.name === 'game'
    },
    isProviderGamePage () {
      return this.$route.name === 'provider.game'
    },
    isPredictionPage () {
      return this.$route.name === 'prediction'
    },
    systemBarEnabled () {
      return config('settings.interface.system_bar.enabled')
    },
    chatEnabled () {
      return config('settings.interface.chat.enabled')
    },
    style () {
      return {
        background: config('settings.theme.backgrounds.page'),
        '--body-font': config('settings.theme.fonts.body.family'),
        '--heading-font': config('settings.theme.fonts.heading.family')
      }
    },
    footerComponent () {
      if (!this.$route.name || this.$route.name === 'home') {
        return false
      }

      return this.$route.name.indexOf('admin.') > -1
        ? 'AdminFooter'
        : 'SecondaryFooter'
    }
  },

  created () {
    this.$store.dispatch('package-manager/fetchOriginalGames')
    this.$store.dispatch('package-manager/fetchProviderGames')
  },

  methods: {
    config,
    setUnreadChatMessagesCount (count) {
      this.unreadChatMessagesCount = count
    },

    linkToAdabet() {
      window.location.href = config('wallet.ada_domain')
    }
  }
}
</script>

<style lang="scss">
body {
  .v-application {

    &, & .title, & .subtitle-1, & .subtitle-2, & .body-1, & .body-2 {
      font-family: var(--body-font), sans-serif !important;
    }

    .text-h1,
    .text-h2,
    .text-h3,
    .text-h4,
    .text-h5,
    .text-h6,
    .text-headline,
    .text-title,
    .text-subtitle-1,
    .text-subtitle-2,
    .text-button,
    .text-caption,
    .text-overline,
    .v-card .headline,
    .v-card .v-toolbar__title {
      font-family: var(--heading-font), sans-serif !important;
    }
  }
}
</style>
