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
use Illuminate\Support\Facades\Schema;

class CreateGameRoomsTable extends Migration
{
    
    public function up()
    {
        Schema::create('game_rooms', function (Blueprint $table) {
            
            $table->id();
            $table->foreignId('user_id');
            $table->string('name', 50);
            $table->string('gameable_type');
            $table->unsignedTinyInteger('status');
            $table->text('parameters');
            $table->timestamps();
            
            $table->foreign('user_id')->references('id')->on('users')->onUpdate('cascade')->onDelete('cascade');
            
            $table->index(['gameable_type', 'status']);
        });
    }

    
    public function down()
    {
        Schema::dropIfExists('game_rooms');
    }
}
