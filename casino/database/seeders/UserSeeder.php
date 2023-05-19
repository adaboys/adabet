<?php

namespace Database\Seeders;

use App\Models\User;
use App\Repositories\UserRepository;
use Carbon\Carbon;
use Illuminate\Database\Seeder;

class UserSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        UserRepository::create([
            'name' => env("ROOT_USER_NAME"),
            'email' => env("ROOT_USER_EMAIL"),
            'role' => User::ROLE_ADMIN,
            'password' => env("ROOT_USER_PASSWORD"),
            'email_verified_at' => Carbon::now(),
        ]);
    }
}
