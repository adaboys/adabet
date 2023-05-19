<script>
export default {
  computed: {
    isMobile () {
      return this.$vuetify.breakpoint.smAndDown
    }
  }
}
</script>
