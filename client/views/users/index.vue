<template>
  <div>
     <h4 class="title">Users</h4>
     <table class="table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Instrument</th>
                <th></th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="user in users">
                <td>
                
                  <router-link :to="{ name: 'User', params: { id: user.Id }}">{{user.Name}}</router-link>
                </td>
                <td>Bass Guitar</td>
                <td>Bass Guitar</td>
                <td >
                </td>
              </tr>
              </tbody>
      </table>
  </div>
</template>
<script>
    export default {
  data () {
    return {
      users: [{Name: "First", Id: "1"}, {Name: "Test", Id: "2"}]
    }
  },
  methods: {
    view () {

    }
  }
}
</script>