<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   database/migrations
 * 
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
*/

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;

class AlterMultiplayerGamesTableChangeTimestamps extends Migration
{
    
    public function up()
    {
        Schema::table('multiplayer_games', function (Blueprint $table) {
            $table->timestamp('start_time', 3)->nullable()->change();
            $table->timestamp('end_time', 3)->nullable()->change();
        });
    }

    
    public function down()
    {
        Schema::table('multiplayer_games', function (Blueprint $table) {
            $table->dateTime('start_time')->change();
            $table->dateTime('end_time')->change();
        });
    }
}
