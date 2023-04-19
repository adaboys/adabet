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

class CreateGamesAssetPredictionTable extends Migration
{
    
    public function up()
    {
        Schema::create('games_asset_prediction', function (Blueprint $table) {
            $table->id();
            $table->foreignId('asset_id')->constrained()->onUpdate('cascade')->onDelete('cascade');
            $table->tinyInteger('direction'); 
            $table->decimal('open_price', 18, 8);
            $table->decimal('close_price', 18, 8)->nullable();
            $table->dateTime('start_time');
            $table->dateTime('end_time');
            $table->timestamps();
            
            $table->index('created_at');
        });
    }

    
    public function down()
    {
        Schema::dropIfExists('games_asset_prediction');
    }
}
