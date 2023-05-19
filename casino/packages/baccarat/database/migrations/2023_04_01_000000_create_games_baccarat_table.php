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

use Illuminate\Support\Facades\Schema;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Database\Migrations\Migration;

class CreateGamesBaccaratTable extends Migration
{
    
    public function up()
    {
        Schema::create('games_baccarat', function (Blueprint $table) {
            
            $table->bigIncrements('id');
            $table->string('deck', 280);
            $table->unsignedTinyInteger('bet_type');
            
            $table->string('player_hand', 30);
            $table->unsignedTinyInteger('player_score');
            
            $table->string('banker_hand', 30);
            $table->unsignedTinyInteger('banker_score');
            $table->timestamps();
        });
    }

    
    public function down()
    {
        Schema::dropIfExists('games_baccarat');
    }
}
