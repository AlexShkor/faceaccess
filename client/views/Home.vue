<template>
  <div class="content has-text-centered">
    <h1> Paralect FaceAccess System</h1>

     <vue-webcam ref='webcam'></vue-webcam>
        <hr/>
        <img :src="photo" alt="" style="width:400px;height:300px" />
        <hr/>
        <button type="button" @click="take_photo">Take Photo</button>
  </div>
</template>

<script>
import VueWebcam from 'components/Webcam'

export default {
  components: { VueWebcam },
  data () {
    return {
      photo: null
    }
  },
  methods: {
    take_photo () {
      this.photo = this.$refs.webcam.getPhoto()
    }
  }
}
</script>

<style lang="scss" scoped>
.is-title {
  text-transform: capitalize;
}
</style>
