<?php
/**
 *   Stake iGaming platform
 *   ----------------------
 *   2022_02_02_000000_alter_users_table_add_code.php
 * 
 *   @copyright  Copyright (c) FinancialPlugins, All rights reserved
 *   @author     FinancialPlugins <info@financialplugins.com>
 *   @see        https://financialplugins.com
*/

use App\Helpers\Utils;
use App\Models\User;
use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class AlterUsersTableAddCode extends Migration
{
    
    public function up()
    {
        Schema::table('users', function (Blueprint $table) {
            $table->string('code', 64)->after('email');
        });

        User::where('code', '')->get()->each(function ($user) {
            $user->update(['code' => Utils::generateRandomString(32)]);
        });

        Schema::table('users', function (Blueprint $table) {
            $table->unique('code');
        });
    }

    
    public function down()
    {
        Schema::table('users', function (Blueprint $table) {
            $table->dropColumn('code');
        });
    }
}
