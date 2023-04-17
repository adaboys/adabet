<?php

namespace Database\Seeders;

use App\Models\User;
use Illuminate\Database\Seeder;

class UserWalletSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        $users = User::query()->where('role', 1)->get();
        foreach ($users as $user) {
            $user->wallet()->create([
                'address' => 'addr_test1vphvppl62gtvaje58376sjddhy794w067pkpwsv6yxqngrq7ggyha',
                'is_primary' => 1,
                'type' => 1,
                'is_active' => 1,
            ]);
        }
    }
}
