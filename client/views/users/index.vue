<template>
  <div>
    <div class="tile is-ancestor">
      <div class="tile is-parent">
        <article class="tile is-child box">
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
                <td>
                </td>
              </tr>
            </tbody>
          </table>
        </article>
      </div>
    </div>
  </div>
</template>

<script>
  import usersDs from 'components/UsersDataService'

  export default {
    data() {
      return {
        users: []
      }
    },
    beforeRouteEnter(to, from, next) {
      usersDs.getAll((err, users) => {
        console.log(users)
        if (err) {
          // display some global error message
          next(false)
        } else {
          next(vm => {
            vm.users = users
          })
        }
      })
    },
    methods: {
      view() {

      }
    }
  }
</script>
